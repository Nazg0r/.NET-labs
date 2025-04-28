import { StudentWork } from './workspace/studentWork.model';

export interface Student {
  id: string;
  username: string;
  name: string;
  surname: string;
  group: string;
  works: StudentWork[];
}
