import { useState } from 'react';
import {
  Paper,
  Typography,
  Box,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Divider,
  Chip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import EmployeeDialog from './EmployeeDialog';
import { employeesApi } from '../api/client';
import type { Employee } from '../types';

interface Props {
  employees: Employee[];
  onRefresh: () => void;
  onSnack: (msg: string, sev?: 'success' | 'error') => void;
}

export default function EmployeeManagement({ employees, onRefresh, onSnack }: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editEmployee, setEditEmployee] = useState<Employee | null>(null);

  const openAdd = () => {
    setEditEmployee(null);
    setDialogOpen(true);
  };

  const openEdit = (emp: Employee) => {
    setEditEmployee(emp);
    setDialogOpen(true);
  };

  const handleSave = async (data: { name: string; role: string }) => {
    try {
      if (editEmployee) {
        await employeesApi.update(editEmployee.id, data);
        onSnack('Employee updated');
      } else {
        await employeesApi.create(data);
        onSnack('Employee added');
      }
      setDialogOpen(false);
      onRefresh();
    } catch {
      onSnack('Failed to save employee', 'error');
    }
  };

  const handleDelete = async (id?: number) => {
    const targetId = id ?? editEmployee?.id;
    if (!targetId) return;
    try {
      await employeesApi.delete(targetId);
      onSnack('Employee deleted');
      setDialogOpen(false);
      onRefresh();
    } catch {
      onSnack('Failed to delete employee', 'error');
    }
  };

  return (
    <>
      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', mt: 3 }}>
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Employees
          </Typography>
          <IconButton color="primary" onClick={openAdd}>
            <AddIcon />
          </IconButton>
        </Box>
        <Divider />
        <List dense sx={{ maxHeight: 400, overflow: 'auto' }}>
          {employees.map((emp) => (
            <ListItem
              key={emp.id}
              secondaryAction={
                <Box>
                  <IconButton size="small" onClick={() => openEdit(emp)}>
                    <EditIcon fontSize="small" />
                  </IconButton>
                  <IconButton size="small" onClick={() => handleDelete(emp.id)}>
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </Box>
              }
            >
              <ListItemText
                primary={
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {emp.name}
                    </Typography>
                    <Chip label={emp.role} size="small" variant="outlined" />
                  </Box>
                }
              />
            </ListItem>
          ))}
          {employees.length === 0 && (
            <ListItem>
              <ListItemText
                primary="No employees yet"
                sx={{ textAlign: 'center', color: 'text.secondary' }}
              />
            </ListItem>
          )}
        </List>
      </Paper>

      <EmployeeDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onSave={handleSave}
        onDelete={editEmployee ? () => handleDelete() : undefined}
        employee={editEmployee}
      />
    </>
  );
}
