import React from 'react';
import classes from './SingleChoice.module.css';

const SingleChoice = (props) => {
  return (
    <div className={classes['single-choice']}>
        <label className={classes['single-choice__label']}>{props.label}</label>
        {props.answers.map(answer=>(
                <div className={classes['single-choice__answer']} key={answer.Id}>
                <input type="radio" id={answer.Id} data-type={props.type} data-q={props.id} data-a={answer.Id} name={'a_'+props.id+'[]'} onChange={props.AnsweredQuestionsHandler}></input>
                <label htmlFor={answer.Id} >{answer.Label}</label>
                </div>
        ))}
    </div>

  );
};

export default SingleChoice;