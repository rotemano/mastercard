import React from 'react';
import logo from './images/Mastercard-logo.png';
import classes from './Header.module.css';

const Header = (props) => {
  return (
    <header className={classes['main-header']}>
      <h1>Mastercard</h1>
      <img src={logo} alt="Logo" className={classes['logo-header']} />;
    </header>
  );
};

export default Header;