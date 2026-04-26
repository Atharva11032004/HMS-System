import { Pipe, PipeTransform } from '@angular/core';
import { UserDto } from '../services/auth';

@Pipe({ name: 'roleCount', standalone: true })
export class RoleCountPipe implements PipeTransform {
  transform(users: UserDto[], role: string): number {
    return users.filter(u => u.role === role).length;
  }
}
