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

export type PersonProps = {
  onChangeName: (value: string) => void;
  onChangeAddress: (value: string) => void;
  onChangePhone: (value: string) => void;
  onChangeAge: (value: number) => void;
  onChangeMail: (value: string) => void;
}

export const Person = (props: PersonProps) => {
  const classes = useStyles();

  const { onChangeName, onChangeAddress, onChangePhone, onChangeAge, onChangeMail } = props;

  return (
    <Box>
      摂取する方の情報を入力してください。

      <form className={classes.container} noValidate>
        <Grid className={classes.root} container spacing={1}>
          <Grid item xs={12}>
            <TextField className={classes.text400} required label="氏名" onChange={event => onChangeName && onChangeName(event.target.value)} />
          </Grid>
          <Grid item xs={12}>
            <TextField required fullWidth label="住所" onChange={event => onChangeAddress && onChangeAddress(event.target.value)} />
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text400} required label="電話" onChange={event => onChangePhone && onChangePhone(event.target.value)} />
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text100} type="number" required label="年齢" onChange={event => onChangeAge && onChangeAge(Number(event.target.value))} />
          </Grid>
          <Grid item xs={12}>
            <TextField className={classes.text400} label="E-mail" onChange={event => onChangeMail && onChangeMail(event.target.value)} />
          </Grid>
        </Grid>
      </form>

    </Box >
  );
}

export default Person;