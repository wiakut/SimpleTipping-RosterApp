export interface Employee {
  id: number;
  name: string;
  role: string;
}

export interface Shift {
  id: number;
  employeeId: number;
  employeeName: string;
  date: string;
  startTime: string;
  endTime: string;
}

export interface ShiftCreate {
  employeeId: number;
  date: string;
  startTime: string;
  endTime: string;
}

export interface EmployeeCreate {
  name: string;
  role: string;
}

export interface TipEntry {
  id: number;
  date: string;
  amount: number;
  source: string;
}

export interface TipEntryCreate {
  date: string;
  amount: number;
  source: string;
}

export interface EmployeeSummary {
  employeeId: number;
  name: string;
  role: string;
  hoursWorked: number;
  tipShare: number;
  percentage: number;
}

export interface WeeklySummary {
  weekStart: string;
  weekEnd: string;
  totalTips: number;
  totalHours: number;
  employees: EmployeeSummary[];
}
