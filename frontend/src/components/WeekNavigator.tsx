import { Box, IconButton, Typography, Chip } from '@mui/material';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import TodayIcon from '@mui/icons-material/Today';
import dayjs from 'dayjs';

interface Props {
  currentWeek: string;
  onChange: (weekStart: string) => void;
}

export default function WeekNavigator({ currentWeek, onChange }: Props) {
  const start = dayjs(currentWeek);
  const end = start.add(6, 'day');
  const isCurrentWeek = start.isSame(dayjs().isoWeekday(1), 'day');

  const navigate = (dir: number) => {
    onChange(start.add(dir * 7, 'day').format('YYYY-MM-DD'));
  };

  const goToday = () => {
    onChange(dayjs().isoWeekday(1).format('YYYY-MM-DD'));
  };

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        gap: 2,
        mb: 3,
      }}
    >
      <IconButton onClick={() => navigate(-1)} size="large">
        <ChevronLeftIcon />
      </IconButton>

      <Box sx={{ textAlign: 'center', minWidth: 260 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          {start.format('D MMM')} — {end.format('D MMM YYYY')}
        </Typography>
        {isCurrentWeek && (
          <Chip label="Current Week" color="primary" size="small" sx={{ mt: 0.5 }} />
        )}
      </Box>

      <IconButton onClick={() => navigate(1)} size="large">
        <ChevronRightIcon />
      </IconButton>

      {!isCurrentWeek && (
        <IconButton onClick={goToday} size="small" title="Go to current week">
          <TodayIcon />
        </IconButton>
      )}
    </Box>
  );
}
