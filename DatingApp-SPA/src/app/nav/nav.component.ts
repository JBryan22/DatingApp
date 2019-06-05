import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import * as alertifyjs from 'alertifyjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    // here just for demonstration i have imported alertifyjs without the service just to show that it works
    // i imagine one of the advantages to creating a service for alertify methods is that we can add additional logic for each method
    // that way success could do additional things if we wanted it to
    // it also supports decoupling in general so that if there is something similar to alertify we can just change the service methods
    this.authService.login(this.model).subscribe(next => {
      alertifyjs.success('Logged in successfully new');
      // this.alertify.success('Logged in successfully');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn() {
    // const token = localStorage.getItem('token');
    // // !!  = if exists it is true otherwise it is false - it coerces the value into its boolean truthiness exquivalent
    // return !!token;
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('Logged out');
  }

}
