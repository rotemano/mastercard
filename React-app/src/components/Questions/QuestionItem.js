import React from 'react'
import MultiChoice from './MultiChoice'
import SingleChoice from './SingleChoice'

const QuestionItem = (props) => {
    const Type_id = props.type;
    if (Type_id===1) 
    return <MultiChoice type={props.type} label={props.label} id={props.id} answers={props.answers} AnsweredQuestionsHandler={props.AnsweredQuestionsHandler} commentHandler={props.commentHandler}></MultiChoice>
    else
    return <SingleChoice type={props.type} label={props.label} id={props.id} answers={props.answers} AnsweredQuestionsHandler={props.AnsweredQuestionsHandler}></SingleChoice>
  };
  
  export default QuestionItem;