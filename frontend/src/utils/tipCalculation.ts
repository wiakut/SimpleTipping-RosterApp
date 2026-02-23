export interface EmployeeHours {
  employeeId: number;
  name: string;
  hours: number;
}

export interface TipSplitResult {
  employeeId: number;
  name: string;
  hours: number;
  tipShare: number;
  percentage: number;
}

export function calculateTipSplit(
  totalTips: number,
  employeeHours: EmployeeHours[]
): TipSplitResult[] {
  const totalHours = employeeHours.reduce((sum, e) => sum + e.hours, 0);

  if (totalHours === 0) {
    return employeeHours.map((e) => ({
      ...e,
      tipShare: 0,
      percentage: 0,
    }));
  }

  return employeeHours.map((e) => {
    const percentage = Math.round((e.hours / totalHours) * 1000) / 10;
    const tipShare = Math.round((e.hours / totalHours) * totalTips * 100) / 100;
    return { ...e, tipShare, percentage };
  });
}

export function calculateShiftHours(startTime: string, endTime: string): number {
  const [sh, sm] = startTime.split(':').map(Number);
  const [eh, em] = endTime.split(':').map(Number);
  return (eh * 60 + em - (sh * 60 + sm)) / 60;
}

export function calculateTotalHours(
  shifts: { startTime: string; endTime: string }[]
): number {
  return shifts.reduce((sum, s) => sum + calculateShiftHours(s.startTime, s.endTime), 0);
}
