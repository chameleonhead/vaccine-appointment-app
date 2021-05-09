import * as React from 'react';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Box from '@material-ui/core/Box';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import Grid from '@material-ui/core/Grid';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    container: {
      display: 'flex',
      flexWrap: 'wrap',
    },
    textField: {
      marginLeft: theme.spacing(1),
      marginRight: theme.spacing(1),
      width: 200,
    },
    root: {
      flexGrow: 1,
    },
  }),
);

export type DateTimeProps = {
  date: string;
  time: string;
  onChangeDate: (value: string) => void;
  onChangeTime: (value: string) => void;
}

export const DateTime = (props: DateTimeProps) => {
  const classes = useStyles();

  const { date, time, onChangeDate, onChangeTime } = props;
  const [selectedTime, setSelectedTime] = React.useState(time);

  const handleClickTime = (t: string) => {
    onChangeTime(t);
    setSelectedTime(t);
  }

  return (
    <Box m={2} display="flex" flexDirection="column" bgcolor="background.paper">
      <p>摂取する日にちと時間帯を選択してください。</p>

      <form className={classes.container} noValidate>
        <div className={classes.root}>
          <Grid container spacing={1}>
            <Grid item xs={12}>
              <TextField type="date" defaultValue={date} className={classes.textField} InputLabelProps={{ shrink: true }} onChange={event => onChangeDate && onChangeDate(event.target.value)} />
            </Grid>

            <Grid item xs={12}>
              <Button variant={selectedTime === "0900" ? ("contained") : ("outlined")} color="primary" fullWidth onClick={() => handleClickTime("0900")}>09:00～10:00</Button>
            </Grid>
            <Grid item xs={12}>
              <Button variant={selectedTime === "1000" ? ("contained") : ("outlined")} color="primary" disabled fullWidth onClick={() => handleClickTime("1000")}>10:00～11:00</Button>
            </Grid>
            <Grid item xs={12}>
              <Button variant={selectedTime === "1100" ? ("contained") : ("outlined")} color="primary" fullWidth onClick={() => handleClickTime("1100")}>11:00～12:00</Button>
            </Grid>
            <Grid item xs={12}>
              <Button variant={selectedTime === "1600" ? ("contained") : ("outlined")} color="primary" fullWidth onClick={() => handleClickTime("1600")}>16:00～17:00</Button>
            </Grid>
            <Grid item xs={12}>
              <Button variant={selectedTime === "1700" ? ("contained") : ("outlined")} color="primary" fullWidth onClick={() => handleClickTime("1700")}>17:00～18:00</Button>
            </Grid>
          </Grid>
        </div>
      </form>
    </Box>
  );
}

export default DateTime;