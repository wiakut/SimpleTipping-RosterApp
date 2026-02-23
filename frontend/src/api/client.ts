import axios from 'axios';
import type {
  Employee,
  Shift,
  ShiftCreate,
  TipEntry,
  TipEntryCreate,
  WeeklySummary,
} from '../types';

const api = axios.create({
  baseURL: '/api',
});

export const employeesApi = {
  getAll: () => api.get<Employee[]>('/employees').then((r) => r.data),
};

export const shiftsApi = {
  getByWeek: (weekStart: string) =>
    api.get<Shift[]>('/shifts', { params: { weekStart } }).then((r) => r.data),
  create: (data: ShiftCreate) =>
    api.post<Shift>('/shifts', data).then((r) => r.data),
  update: (id: number, data: ShiftCreate) =>
    api.put<Shift>(`/shifts/${id}`, data).then((r) => r.data),
  delete: (id: number) => api.delete(`/shifts/${id}`),
};

export const tipsApi = {
  getByWeek: (weekStart: string) =>
    api.get<TipEntry[]>('/tips', { params: { weekStart } }).then((r) => r.data),
  create: (data: TipEntryCreate) =>
    api.post<TipEntry>('/tips', data).then((r) => r.data),
  update: (id: number, data: TipEntryCreate) =>
    api.put<TipEntry>(`/tips/${id}`, data).then((r) => r.data),
  delete: (id: number) => api.delete(`/tips/${id}`),
};

export const summaryApi = {
  getWeekly: (weekStart: string) =>
    api
      .get<WeeklySummary>('/weekly-summary', { params: { weekStart } })
      .then((r) => r.data),
};
