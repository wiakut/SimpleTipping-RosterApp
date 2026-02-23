import { useState, useEffect, useCallback } from 'react';
import {
  ThemeProvider,
  createTheme,
  CssBaseline,
  Container,
  AppBar,
  Toolbar,
  Typography,
  Box,
  Alert,
  Snackbar,
} from '@mui/material';
import RestaurantIcon from '@mui/icons-material/Restaurant';
import dayjs from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import WeekNavigator from './components/WeekNavigator';
import RosterTable from './components/RosterTable';
import TipsSummary from './components/TipsSummary';
import TipSplitTable from './components/TipSplitTable';
import { employeesApi, shiftsApi, tipsApi, summaryApi } from './api/client';
import type { Employee, Shift, TipEntry, WeeklySummary } from './types';

dayjs.extend(isoWeek);

const theme = createTheme({
  palette: {
    primary: { main: '#1565c0' },
    secondary: { main: '#ff8f00' },
    background: { default: '#f5f5f5' },
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  },
  shape: { borderRadius: 10 },
});

function getWeekStart(date: dayjs.Dayjs): string {
  return date.isoWeekday(1).format('YYYY-MM-DD');
}

export default function App() {
  const [currentWeek, setCurrentWeek] = useState(() =>
    getWeekStart(dayjs())
  );
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [shifts, setShifts] = useState<Shift[]>([]);
  const [tips, setTips] = useState<TipEntry[]>([]);
  const [summary, setSummary] = useState<WeeklySummary | null>(null);
  const [snack, setSnack] = useState<{ open: boolean; message: string; severity: 'success' | 'error' }>({
    open: false,
    message: '',
    severity: 'success',
  });

  const showSnack = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnack({ open: true, message, severity });
  };

  const loadData = useCallback(async () => {
    try {
      const [emps, sh, ti, sum] = await Promise.all([
        employeesApi.getAll(),
        shiftsApi.getByWeek(currentWeek),
        tipsApi.getByWeek(currentWeek),
        summaryApi.getWeekly(currentWeek),
      ]);
      setEmployees(emps);
      setShifts(sh);
      setTips(ti);
      setSummary(sum);
    } catch {
      showSnack('Failed to load data', 'error');
    }
  }, [currentWeek]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleWeekChange = (weekStart: string) => {
    setCurrentWeek(weekStart);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <RestaurantIcon sx={{ mr: 1.5 }} />
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            The Golden Fork
          </Typography>
          <Typography variant="body2" sx={{ ml: 2, opacity: 0.8 }}>
            Tipping & Roster
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="xl" sx={{ mt: 3, mb: 4 }}>
        <WeekNavigator currentWeek={currentWeek} onChange={handleWeekChange} />

        <RosterTable
          employees={employees}
          shifts={shifts}
          weekStart={currentWeek}
          onRefresh={loadData}
          onSnack={showSnack}
        />

        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', md: '1fr 2fr' },
            gap: 3,
            mt: 3,
          }}
        >
          <TipsSummary
            tips={tips}
            weekStart={currentWeek}
            onRefresh={loadData}
            onSnack={showSnack}
          />
          <TipSplitTable summary={summary} />
        </Box>
      </Container>

      <Snackbar
        open={snack.open}
        autoHideDuration={3000}
        onClose={() => setSnack((s) => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          severity={snack.severity}
          onClose={() => setSnack((s) => ({ ...s, open: false }))}
          variant="filled"
        >
          {snack.message}
        </Alert>
      </Snackbar>
    </ThemeProvider>
  );
}
