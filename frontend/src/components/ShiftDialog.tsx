import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box,
} from '@mui/material';
import type { Employee, Shift } from '../types';

interface Props {
  open: boolean;
  onClose: () => void;
  onSave: (data: {
    employeeId: number;
    date: string;
    startTime: string;
    endTime: string;
  }) => void;
  onDelete?: () => void;
  employees: Employee[];
  date: string;
  shift?: Shift | null;
}

export default function ShiftDialog({
  open,
  onClose,
  onSave,
  onDelete,
  employees,
  date,
  shift,
}: Props) {
  const [employeeId, setEmployeeId] = useState<number>(0);
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('17:00');

  useEffect(() => {
    if (shift) {
      setEmployeeId(shift.employeeId);
      setStartTime(shift.startTime.substring(0, 5));
      setEndTime(shift.endTime.substring(0, 5));
    } else {
      setEmployeeId(employees[0]?.id ?? 0);
      setStartTime('09:00');
      setEndTime('17:00');
    }
  }, [shift, employees, open]);

  const handleSubmit = () => {
    if (!employeeId || !startTime || !endTime) return;
    onSave({
      employeeId,
      date,
      startTime: startTime + ':00',
      endTime: endTime + ':00',
    });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{shift ? 'Edit Shift' : 'Add Shift'}</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <TextField
            select
            label="Employee"
            value={employeeId}
            onChange={(e) => setEmployeeId(Number(e.target.value))}
            disabled={!!shift}
            fullWidth
          >
            {employees.map((emp) => (
              <MenuItem key={emp.id} value={emp.id}>
                {emp.name} ({emp.role})
              </MenuItem>
            ))}
          </TextField>
          <TextField
            label="Date"
            type="date"
            value={date}
            disabled
            fullWidth
            slotProps={{ inputLabel: { shrink: true } }}
          />
          <TextField
            label="Start Time"
            type="time"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            fullWidth
            slotProps={{ inputLabel: { shrink: true } }}
          />
          <TextField
            label="End Time"
            type="time"
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
            fullWidth
            slotProps={{ inputLabel: { shrink: true } }}
          />
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        {shift && onDelete && (
          <Button color="error" onClick={onDelete} sx={{ mr: 'auto' }}>
            Delete
          </Button>
        )}
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit}>
          {shift ? 'Update' : 'Add'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
