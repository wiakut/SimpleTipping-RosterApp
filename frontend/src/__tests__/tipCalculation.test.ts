import { describe, it, expect } from 'vitest';
import {
  calculateTipSplit,
  calculateShiftHours,
  calculateTotalHours,
} from '../utils/tipCalculation';

describe('calculateTipSplit', () => {
  it('splits proportionally between two employees', () => {
    const result = calculateTipSplit(300, [
      { employeeId: 1, name: 'Alice', hours: 20 },
      { employeeId: 2, name: 'Bob', hours: 10 },
    ]);

    expect(result[0].tipShare).toBe(200);
    expect(result[1].tipShare).toBe(100);
  });

  it('gives all tips to single employee', () => {
    const result = calculateTipSplit(500, [
      { employeeId: 1, name: 'Alice', hours: 40 },
    ]);

    expect(result[0].tipShare).toBe(500);
    expect(result[0].percentage).toBe(100);
  });

  it('returns zero when no hours worked', () => {
    const result = calculateTipSplit(300, [
      { employeeId: 1, name: 'Alice', hours: 0 },
      { employeeId: 2, name: 'Bob', hours: 0 },
    ]);

    expect(result.every((e) => e.tipShare === 0)).toBe(true);
  });

  it('returns zero tips when total is zero', () => {
    const result = calculateTipSplit(0, [
      { employeeId: 1, name: 'Alice', hours: 20 },
    ]);

    expect(result[0].tipShare).toBe(0);
  });

  it('handles three-way split correctly', () => {
    const result = calculateTipSplit(400, [
      { employeeId: 1, name: 'Alice', hours: 10 },
      { employeeId: 2, name: 'Bob', hours: 10 },
      { employeeId: 3, name: 'Carol', hours: 20 },
    ]);

    expect(result[0].tipShare).toBe(100);
    expect(result[1].tipShare).toBe(100);
    expect(result[2].tipShare).toBe(200);
  });

  it('handles uneven split with rounding', () => {
    const result = calculateTipSplit(100, [
      { employeeId: 1, name: 'Alice', hours: 10 },
      { employeeId: 2, name: 'Bob', hours: 10 },
      { employeeId: 3, name: 'Carol', hours: 10 },
    ]);

    expect(result[0].tipShare).toBe(33.33);
  });

  it('employee with zero hours gets nothing', () => {
    const result = calculateTipSplit(200, [
      { employeeId: 1, name: 'Alice', hours: 20 },
      { employeeId: 2, name: 'Bob', hours: 0 },
    ]);

    expect(result[0].tipShare).toBe(200);
    expect(result[1].tipShare).toBe(0);
  });
});

describe('calculateShiftHours', () => {
  it('calculates a standard 8-hour shift', () => {
    expect(calculateShiftHours('09:00', '17:00')).toBe(8);
  });

  it('calculates a half-hour shift correctly', () => {
    expect(calculateShiftHours('10:00', '14:30')).toBe(4.5);
  });

  it('calculates evening shift', () => {
    expect(calculateShiftHours('16:00', '23:30')).toBe(7.5);
  });
});

describe('calculateTotalHours', () => {
  it('sums multiple shifts', () => {
    const total = calculateTotalHours([
      { startTime: '09:00', endTime: '13:00' },
      { startTime: '14:00', endTime: '18:00' },
    ]);
    expect(total).toBe(8);
  });

  it('returns zero for empty array', () => {
    expect(calculateTotalHours([])).toBe(0);
  });
});
