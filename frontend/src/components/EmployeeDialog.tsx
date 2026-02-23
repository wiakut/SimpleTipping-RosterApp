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
import type { Employee } from '../types';

const ROLES = ['Waiter', 'Bartender', 'Host', 'Chef', 'Manager', 'Busser'];

interface Props {
  open: boolean;
  onClose: () => void;
  onSave: (data: { name: string; role: string }) => void;
  onDelete?: () => void;
  employee?: Employee | null;
}

export default function EmployeeDialog({
  open,
  onClose,
  onSave,
  onDelete,
  employee,
}: Props) {
  const [name, setName] = useState('');
  const [role, setRole] = useState(ROLES[0]);

  useEffect(() => {
    if (employee) {
      setName(employee.name);
      setRole(employee.role);
    } else {
      setName('');
      setRole(ROLES[0]);
    }
  }, [employee, open]);

  const handleSubmit = () => {
    if (!name.trim() || !role.trim()) return;
    onSave({ name: name.trim(), role: role.trim() });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{employee ? 'Edit Employee' : 'Add Employee'}</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <TextField
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            fullWidth
            autoFocus
          />
          <TextField
            select
            label="Role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            fullWidth
          >
            {ROLES.map((r) => (
              <MenuItem key={r} value={r}>
                {r}
              </MenuItem>
            ))}
          </TextField>
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        {employee && onDelete && (
          <Button color="error" onClick={onDelete} sx={{ mr: 'auto' }}>
            Delete
          </Button>
        )}
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit}>
          {employee ? 'Update' : 'Add'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
