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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Chip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import dayjs from 'dayjs';
import { tipsApi } from '../api/client';
import type { TipEntry } from '../types';

const SOURCES = ['Card tips', 'Cash tips', 'Brunch tips', 'Private event gratuity'];

interface Props {
  tips: TipEntry[];
  weekStart: string;
  onRefresh: () => void;
  onSnack: (msg: string, sev?: 'success' | 'error') => void;
}

export default function TipsSummary({ tips, weekStart, onRefresh, onSnack }: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editTip, setEditTip] = useState<TipEntry | null>(null);
  const [date, setDate] = useState('');
  const [amount, setAmount] = useState('');
  const [source, setSource] = useState('Card tips');

  const totalTips = tips.reduce((sum, t) => sum + t.amount, 0);

  const openAdd = () => {
    setEditTip(null);
    setDate(dayjs(weekStart).format('YYYY-MM-DD'));
    setAmount('');
    setSource('Card tips');
    setDialogOpen(true);
  };

  const openEdit = (tip: TipEntry) => {
    setEditTip(tip);
    setDate(tip.date);
    setAmount(tip.amount.toString());
    setSource(tip.source);
    setDialogOpen(true);
  };

  const handleSave = async () => {
    const amt = parseFloat(amount);
    if (isNaN(amt) || amt <= 0) {
      onSnack('Enter a valid amount', 'error');
      return;
    }
    try {
      if (editTip) {
        await tipsApi.update(editTip.id, { date, amount: amt, source });
        onSnack('Tip entry updated');
      } else {
        await tipsApi.create({ date, amount: amt, source });
        onSnack('Tip entry added');
      }
      setDialogOpen(false);
      onRefresh();
    } catch {
      onSnack('Failed to save tip entry', 'error');
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await tipsApi.delete(id);
      onSnack('Tip entry deleted');
      onRefresh();
    } catch {
      onSnack('Failed to delete tip entry', 'error');
    }
  };

  const grouped = tips.reduce<Record<string, TipEntry[]>>((acc, t) => {
    (acc[t.date] ??= []).push(t);
    return acc;
  }, {});

  return (
    <>
      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', height: 'fit-content' }}>
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Box>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              Tips This Week
            </Typography>
            <Typography variant="h4" color="secondary" sx={{ fontWeight: 800 }}>
              &euro;{totalTips.toFixed(2)}
            </Typography>
          </Box>
          <IconButton color="primary" onClick={openAdd}>
            <AddIcon />
          </IconButton>
        </Box>
        <Divider />
        <List dense sx={{ maxHeight: 360, overflow: 'auto' }}>
          {Object.entries(grouped)
            .sort(([a], [b]) => a.localeCompare(b))
            .map(([d, entries]) => (
              <Box key={d}>
                <ListItem sx={{ pb: 0 }}>
                  <Typography variant="caption" color="text.secondary" sx={{ fontWeight: 700 }}>
                    {dayjs(d).format('ddd D MMM')}
                  </Typography>
                </ListItem>
                {entries.map((tip) => (
                  <ListItem
                    key={tip.id}
                    secondaryAction={
                      <Box>
                        <IconButton size="small" onClick={() => openEdit(tip)}>
                          <EditIcon fontSize="small" />
                        </IconButton>
                        <IconButton size="small" onClick={() => handleDelete(tip.id)}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Box>
                    }
                  >
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="body2" sx={{ fontWeight: 600 }}>
                            &euro;{tip.amount.toFixed(2)}
                          </Typography>
                          <Chip label={tip.source} size="small" variant="outlined" />
                        </Box>
                      }
                    />
                  </ListItem>
                ))}
              </Box>
            ))}
          {tips.length === 0 && (
            <ListItem>
              <ListItemText
                primary="No tip entries for this week"
                sx={{ textAlign: 'center', color: 'text.secondary' }}
              />
            </ListItem>
          )}
        </List>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>{editTip ? 'Edit Tip Entry' : 'Add Tip Entry'}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField
              label="Date"
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
              fullWidth
              slotProps={{ inputLabel: { shrink: true } }}
            />
            <TextField
              label="Amount (EUR)"
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              fullWidth
              slotProps={{ input: { inputProps: { min: 0, step: 0.01 } } }}
            />
            <TextField
              select
              label="Source"
              value={source}
              onChange={(e) => setSource(e.target.value)}
              fullWidth
            >
              {SOURCES.map((s) => (
                <MenuItem key={s} value={s}>
                  {s}
                </MenuItem>
              ))}
            </TextField>
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>
            {editTip ? 'Update' : 'Add'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
