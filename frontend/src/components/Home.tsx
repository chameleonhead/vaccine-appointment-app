import React from 'react';
import { connect } from 'react-redux';
import { makeStyles, Theme, createStyles, withStyles } from '@material-ui/core/styles';
import clsx from 'clsx';
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import CalendarTodayIcon from '@material-ui/icons/CalendarToday';
import PersonIcon from '@material-ui/icons/Person';
import DoneIcon from '@material-ui/icons/Done';
import StepConnector from '@material-ui/core/StepConnector';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import { StepIconProps } from '@material-ui/core/StepIcon';
import DateTime from './DateTime';
import Person from './Person';
import Doing from './Doing';

const ColorlibConnector = withStyles({
  alternativeLabel: {
    top: 22,
  },
  active: {
    '& $line': {
      backgroundImage:
        'linear-gradient( 95deg,rgb(33,113,242) 0%,rgb(87,64,233) 50%,rgb(135,35,138) 100%)',
    },
  },
  completed: {
    '& $line': {
      backgroundImage:
        'linear-gradient( 95deg,rgb(33,113,242) 0%,rgb(87,64,233) 50%,rgb(135,35,138) 100%)',
    },
  },
  line: {
    height: 3,
    border: 0,
    backgroundColor: '#eaeaf0',
    borderRadius: 1,
  },
})(StepConnector);

const useColorlibStepIconStyles = makeStyles({
  root: {
    backgroundColor: '#ccc',
    zIndex: 1,
    color: '#fff',
    width: 50,
    height: 50,
    display: 'flex',
    borderRadius: '50%',
    justifyContent: 'center',
    alignItems: 'center',
  },
  active: {
    backgroundImage:
      'linear-gradient( 136deg, rgb(33,113,242) 0%, rgb(87,64,233) 50%, rgb(135,35,138) 100%)',
    boxShadow: '0 4px 10px 0 rgba(0,0,0,.25)',
  },
  completed: {
    backgroundImage:
      'linear-gradient( 136deg, rgb(33,113,242) 0%, rgb(87,64,233) 50%, rgb(135,35,138) 100%)',
  },
});

function ColorlibStepIcon(props: StepIconProps) {
  const classes = useColorlibStepIconStyles();
  const { active, completed } = props;

  const icons: { [index: string]: React.ReactElement } = {
    1: <CalendarTodayIcon />,
    2: <PersonIcon />,
    3: <DoneIcon />,
  };

  return (
    <div
      className={clsx(classes.root, {
        [classes.active]: active,
        [classes.completed]: completed,
      })}
    >
      {icons[String(props.icon)]}
    </div>
  );
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      width: '100%',
    },
    button: {
      marginRight: theme.spacing(1),
    },
    instructions: {
      marginTop: theme.spacing(1),
      marginBottom: theme.spacing(1),
    },
  }),
);

function getSteps() {
  return ["日程を選択する", "接種者の情報を入力する", "予約を確定する"];
}

function getToday() {
  const now = new Date();
  return `${now.getFullYear()}-${now.getMonth() + 1 < 10 ? "0" + (now.getMonth() + 1) : now.getMonth() + 1}-${now.getDate() < 10 ? "0" + now.getDate() : now.getDate()}`;
}

function Home() {
  const classes = useStyles();
  const [activeStep, setActiveStep] = React.useState(0);
  const steps = getSteps();
  const [reservation, setReservation] = React.useState({ date: getToday(), time: "0900", name: "", address: "", phone: "", age: 0, mail: "" });

  const handleNext = () => { setActiveStep((prevActiveStep) => prevActiveStep + 1); };
  const handleBack = () => { setActiveStep((prevActiveStep) => prevActiveStep - 1); };
  const handleReset = () => { setActiveStep(0); };

  return (
    <div className={classes.root}>
      { activeStep !== steps.length && (
        <Stepper alternativeLabel activeStep={activeStep} connector={<ColorlibConnector />}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel StepIconComponent={ColorlibStepIcon}>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>
      )}
      <div>
        {activeStep === steps.length ? (
          <div>
            <Typography className={classes.instructions}>予約が完了しました。</Typography>
            <Button onClick={handleReset} className={classes.button}>Reset</Button>
          </div>
        ) : activeStep === 0 ? (
          <div>
            <DateTime date={reservation.date} time={reservation.time} onChangeDate={value => setReservation({ ...reservation, date: value })}
              onChangeTime={value => setReservation({ ...reservation, time: value })} />
          </div>
        ) : activeStep === 1 ? (
          <div>
            <Person onChangeName={value => setReservation({ ...reservation, name: value })} onChangeAddress={value => setReservation({ ...reservation, address: value })}
              onChangePhone={value => setReservation({ ...reservation, phone: value })} onChangeAge={value => setReservation({ ...reservation, age: value })} onChangeMail={value => setReservation({ ...reservation, mail: value })} />
          </div>
        ) : activeStep === 2 ? (
          <div>
            <Doing date={reservation.date} time={reservation.time} />
          </div>
        ) : (<div></div>)}
        <div>
          <Button disabled={activeStep === 0} onClick={handleBack} className={classes.button}>戻る</Button>
          <Button variant="contained" color="primary" onClick={handleNext} className={classes.button}>
            {activeStep === steps.length - 1 ? '予約を確定する' : '次へ'}
          </Button>
        </div>
      </div>
      {reservation.date} {reservation.time} {reservation.name} {reservation.address} {reservation.phone} {reservation.age} {reservation.mail}

    </div>
  );
}

export default connect()(Home);
