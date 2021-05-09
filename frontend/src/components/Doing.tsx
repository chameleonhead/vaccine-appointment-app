import * as React from 'react';
import Box from '@material-ui/core/Box';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    container: {
      display: 'flex',
      flexWrap: 'wrap',
      '& .MuiTextField-root': {
        margin: theme.spacing(1)
      },
    },
    root: {
      flexGrow: 1,
    },
    text400: {
      width: 400
    },
    text100: {
      width: 100
    }
  }),
);

export type DoingProps = {
  date: string;
  time: string;
}

export const Doing = (props: DoingProps) => {
  const classes = useStyles();

  const { date, time } = props;

  return (
    <Box>
      以下の内容で予約を確定します。
      <form className={classes.container} noValidate>
        <Grid className={classes.root} container spacing={1}>
          <Grid item xs={12}>
            <TextField className={classes.text400} required label="氏名" defaultValue={date} InputProps={{ readOnly: true }} />
          </Grid>
          <Grid item xs={12}>
            <TextField required fullWidth label="住所" InputProps={{ readOnly: true }}/>
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text400} required label="電話" InputProps={{ readOnly: true }}/>
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text100} type="number" label="年齢" InputProps={{ readOnly: true }}/>
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text400} label="E-mail" InputProps={{ readOnly: true }}/>
          </Grid>
        </Grid>
      </form>
    </Box>
  )
}

export default Doing;