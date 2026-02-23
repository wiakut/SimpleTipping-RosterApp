import { useState } from 'react';
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  Box,
  Chip,
  IconButton,
  Tooltip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import dayjs from 'dayjs';
import ShiftDialog from './ShiftDialog';
import { shiftsApi } from '../api/client';
import type { Employee, Shift } from '../types';

const DAYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

interface Props {
  employees: Employee[];
  shifts: Shift[];
  weekStart: string;
  onRefresh: () => void;
  onSnack: (msg: string, sev?: 'success' | 'error') => void;
}

function formatTime(t: string) {
  return t.substring(0, 5);
}

function hoursForShift(s: Shift): number {
  const [sh, sm] = s.startTime.split(':').map(Number);
  const [eh, em] = s.endTime.split(':').map(Number);
  return (eh * 60 + em - sh * 60 - sm) / 60;
}

export default function RosterTable({
  employees,
  shifts,
  weekStart,
  onRefresh,
  onSnack,
}: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogDate, setDialogDate] = useState('');
  const [dialogShift, setDialogShift] = useState<Shift | null>(null);
  const [dialogEmployeeFilter, setDialogEmployeeFilter] = useState<number | null>(null);

  const dates = Array.from({ length: 7 }, (_, i) =>
    dayjs(weekStart).add(i, 'day').format('YYYY-MM-DD')
  );

  const getShift = (empId: number, date: string) =>
    shifts.find((s) => s.employeeId === empId && s.date === date);

  const totalHours = (empId: number) =>
    shifts
      .filter((s) => s.employeeId === empId)
      .reduce((sum, s) => sum + hoursForShift(s), 0);

  const openAdd = (date: string, empId?: number) => {
    setDialogShift(null);
    setDialogDate(date);
    setDialogEmployeeFilter(empId ?? null);
    setDialogOpen(true);
  };

  const openEdit = (shift: Shift) => {
    setDialogShift(shift);
    setDialogDate(shift.date);
    setDialogEmployeeFilter(null);
    setDialogOpen(true);
  };

  const handleSave = async (data: {
    employeeId: number;
    date: string;
    startTime: string;
    endTime: string;
  }) => {
    try {
      if (dialogShift) {
        await shiftsApi.update(dialogShift.id, data);
        onSnack('Shift updated');
      } else {
        await shiftsApi.create(data);
        onSnack('Shift added');
      }
      setDialogOpen(false);
      onRefresh();
    } catch {
      onSnack('Failed to save shift', 'error');
    }
  };

  const handleDelete = async () => {
    if (!dialogShift) return;
    try {
      await shiftsApi.delete(dialogShift.id);
      onSnack('Shift deleted');
      setDialogOpen(false);
      onRefresh();
    } catch {
      onSnack('Failed to delete shift', 'error');
    }
  };

  const filteredEmployees = dialogEmployeeFilter
    ? employees.filter((e) => e.id === dialogEmployeeFilter)
    : employees;

  return (
    <>
      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Weekly Roster
          </Typography>
        </Box>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700, minWidth: 160 }}>Employee</TableCell>
                {dates.map((d, i) => (
                  <TableCell key={d} align="center" sx={{ fontWeight: 700, minWidth: 110 }}>
                    <Box>
                      {DAYS[i]}
                      <Typography variant="caption" display="block" color="text.secondary">
                        {dayjs(d).format('D MMM')}
                      </Typography>
                    </Box>
                  </TableCell>
                ))}
                <TableCell align="center" sx={{ fontWeight: 700, minWidth: 80 }}>
                  Total
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.map((emp) => (
                <TableRow key={emp.id} hover>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {emp.name}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {emp.role}
                    </Typography>
                  </TableCell>
                  {dates.map((d) => {
                    const shift = getShift(emp.id, d);
                    return (
                      <TableCell key={d} align="center" sx={{ py: 1 }}>
                        {shift ? (
                          <Chip
                            label={`${formatTime(shift.startTime)} – ${formatTime(shift.endTime)}`}
                            size="small"
                            color="primary"
                            variant="outlined"
                            onClick={() => openEdit(shift)}
                            sx={{ cursor: 'pointer', fontSize: '0.75rem' }}
                          />
                        ) : (
                          <Tooltip title="Add shift">
                            <IconButton
                              size="small"
                              onClick={() => openAdd(d, emp.id)}
                              sx={{ opacity: 0.3, '&:hover': { opacity: 1 } }}
                            >
                              <AddIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                      </TableCell>
                    );
                  })}
                  <TableCell align="center">
                    <Typography variant="body2" sx={{ fontWeight: 700 }}>
                      {totalHours(emp.id).toFixed(1)}h
                    </Typography>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      <ShiftDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onSave={handleSave}
        onDelete={dialogShift ? handleDelete : undefined}
        employees={filteredEmployees}
        date={dialogDate}
        shift={dialogShift}
      />
    </>
  );
}
