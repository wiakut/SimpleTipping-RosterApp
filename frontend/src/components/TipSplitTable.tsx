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
  LinearProgress,
} from '@mui/material';
import type { WeeklySummary } from '../types';

interface Props {
  summary: WeeklySummary | null;
}

export default function TipSplitTable({ summary }: Props) {
  if (!summary) return null;

  const maxHours = Math.max(...summary.employees.map((e) => e.hoursWorked), 1);

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
      <Box sx={{ p: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          Tip Split
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Total: &euro;{summary.totalTips.toFixed(2)} across{' '}
          {summary.totalHours.toFixed(1)} hours
        </Typography>
      </Box>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 700 }}>Employee</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Role</TableCell>
              <TableCell align="right" sx={{ fontWeight: 700 }}>
                Hours
              </TableCell>
              <TableCell sx={{ fontWeight: 700, minWidth: 120 }}>Distribution</TableCell>
              <TableCell align="right" sx={{ fontWeight: 700 }}>
                Share
              </TableCell>
              <TableCell align="right" sx={{ fontWeight: 700 }}>
                Tip Amount
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {summary.employees
              .filter((e) => e.hoursWorked > 0)
              .sort((a, b) => b.tipShare - a.tipShare)
              .map((emp) => (
                <TableRow key={emp.employeeId} hover>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {emp.name}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {emp.role}
                    </Typography>
                  </TableCell>
                  <TableCell align="right">{emp.hoursWorked.toFixed(1)}h</TableCell>
                  <TableCell>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <LinearProgress
                        variant="determinate"
                        value={(emp.hoursWorked / maxHours) * 100}
                        sx={{ flexGrow: 1, height: 8, borderRadius: 4 }}
                      />
                    </Box>
                  </TableCell>
                  <TableCell align="right">{emp.percentage.toFixed(1)}%</TableCell>
                  <TableCell align="right">
                    <Typography
                      variant="body2"
                      sx={{ fontWeight: 700, color: 'secondary.main' }}
                    >
                      &euro;{emp.tipShare.toFixed(2)}
                    </Typography>
                  </TableCell>
                </TableRow>
              ))}
            {summary.employees.every((e) => e.hoursWorked === 0) && (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography variant="body2" color="text.secondary" sx={{ py: 2 }}>
                    No hours recorded for this week
                  </Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  );
}
