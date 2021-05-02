import * as React from 'react';
import { Helmet } from 'react-helmet';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export default class Layout extends React.Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <Helmet>
          <meta charSet="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
          <meta name="theme-color" content="#000000" />
          <title>VaccineAppointment.Web</title>
        </Helmet>
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
