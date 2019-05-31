import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';

  // once again we create a service to logically seperate our application
  // the reasoning is similar to having a repository in .net
  // this service will handle talking to our API and retrieving data in a centralized way
  // any component that needs the data that this service provides can use the methods in this service for that purpose
  // rather than implementing those methods in their own component and potentially causing code duplication
  constructor(private http: HttpClient) { }

  login(model: any) {
    // sending api url + /login and our model as body. model should be a json
    // pipe() is an rxjs method that allows you to combine js methods together

    // storing jwt inside localStorage (which is storage for a browser window domain and persists after page close)
    // is a possible security flaw worth looking up at some point later
    // using cookies like this is fairly standard i think, but there are additional steps to make it secure
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

}
