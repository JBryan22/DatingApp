using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers {
    [Authorize]
    [Route ("api/users/{userid}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController (IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userid, PhotoForCreationDto photoForCreationDto) 
        {
            // simply checking to see if the person logged on is allowed to upload photos for this user
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(userid);

            // the dto has a inMemory file as one of its properties IFormFile
            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // using statements are used because we want it to automatically dispose of the object after it has been used
                // in this case stream will be disposed of after it has been used, while the data retrieved from the stream
                // will remain in variables/objects created outside of the using statement (uploadResult in this case)
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            // check to see if any photos exist, if not make main
            if (!userFromRepo.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not add the photo");
        }

    }
}