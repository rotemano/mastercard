import React from 'react';
import classes from './MultiChoice.module.css';

const MultiChoice = (props) => {
  return (
    <div className={classes['multi-choice']}>
        <label className={classes['multi-choice__label']}>{props.label}</label>
        {props.answers.map(answer=>(
                <div className={classes['multi-choice__answer']} key={answer.Id}>
                <input type="checkbox" id={answer.Id} data-type={props.type} data-q={props.id} data-a={answer.Id} name={'a_'+props.id+'[]'}  onChange={props.AnsweredQuestionsHandler}></input>
                <label htmlFor={answer.Id} >{answer.Label}</label>
                </div>
        ))}
         <label className={classes['multi-choice__comment']} htmlFor={props.id+'_c'}> Comment:</label>
         <input type="text" data-q={props.id} onBlur={props.commentHandler}></input>

    </div>
  );
};

export default MultiChoice;