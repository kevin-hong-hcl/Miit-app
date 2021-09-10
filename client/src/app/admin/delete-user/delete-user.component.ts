import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-delete-user',
  templateUrl: './delete-user.component.html',
  styleUrls: ['./delete-user.component.css']
})
export class DeleteUserComponent implements OnInit {
  users: Partial<User[]>;

  constructor(private adminService: AdminService, private accountService: AccountService, 
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    })
  }

  deleteUser(user: User){
    this.accountService.deleteUser(user.username).subscribe(() => {
      this.toastr.success("Account deleted")
    });
    window.location.reload();
  }

}
