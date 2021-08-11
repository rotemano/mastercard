import React from 'react';
import styled from './Button.module.css'

const Button=(props)=>{

return (
    <button type={props.type} onClick={props.onClick} className={styled.button}>{props.children}</button>
)


}
export default Button;