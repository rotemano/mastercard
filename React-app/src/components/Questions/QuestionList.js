import React,{useState} from 'react'
import classes from './QuestionList.module.css';
import QuestionItem from './QuestionItem'
import Button from '../UI/Button/Button'

const QUESTION_TYPE = {
    MULTIPLE: '1',
    SINGLE: '2'
}

const QuestionList = (props) => {
    const [showErrorMessage, setShowErrorMessage] = useState(false);
    const [answeredQuestions,setAnsweredQuestions]=useState([]);
    const [answersCount, setAnswersCount] = useState(0)
    const [showScore,setShowScore]=useState();
    const [score,setScore]=useState();

    const submitHandler=(event)=>{
        event.preventDefault();
        var isValidForm=true;
        isValidForm = answersCount === props.items.length

        if (!isValidForm) {
            setShowErrorMessage(true)
            return;
        }
        else {
            setShowErrorMessage(false);
            sendDataToServer(props.portNumber.toString());
        }
    }


//send questions and answer to server
    const sendDataToServer=(port)=>{
        const data={
            user_id:1,
            "questions": answeredQuestions
        }
        var URL="http://localhost:"+port+"/api/questions";
        fetch(URL,{
            method: 'POST',
            mode: 'cors',
            credentials: 'same-origin',
            headers: {
              'Content-Type': 'application/json', 
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify(data)
        })
            .then(response => {
              return response.json();
            })
            .then(
              (data) => {
                  if (data!=-1){
                    setScore(data);
                    setShowScore(true);
                  }else{
                    setScore("NA, problem occure while processing the score");
                  }
                
              },
              (error) => {
                console.log(error)
              }
            )
    }


const commentHandler=(event)=>{
    const element = event.target;
    const q_id = element.dataset.q;
    const comment_text = element.value
    
        if (comment_text) {
            const questionObj = answeredQuestions.find(question => question.id === q_id)

            if (!questionObj) {
             answeredQuestions.push({ id: q_id, answers: [], comment: comment_text })   
            } else {
                questionObj.comment = comment_text
            }
        }

        setAnsweredQuestions(answeredQuestions)
    }

    const AnsweredQuestionsHandler=(event)=>{
        const element=event.target;
        const q_type = element.dataset.type;
        const q_id= element.dataset.q;
        const a_id= element.dataset.a;
        const questionObj = answeredQuestions.find(question => question.id === q_id)
        const isAdded = element.checked

        if (isAdded) {
            // on answer added
            if (!questionObj){
                answeredQuestions.push({id:q_id, answers: [{ id: a_id }],comment:"" })
            } else {
                if (q_type === QUESTION_TYPE.SINGLE) {
                    questionObj.answers = [{ id: a_id }]
                } else {
                    questionObj.answers.push({ id: a_id })
                }
            }
        } else {
            // on answer removed
            const answerIndex = questionObj.answers.findIndex(answer => answer.id === a_id)
            questionObj.answers.splice(answerIndex, 1)
        }

        setAnswersCount(answeredQuestions.filter(q => q.answers.length).length)
        setAnsweredQuestions(answeredQuestions)
    }

    if (props.items.length === 0)
    {
        return <h2 className={classes['question-list__error']}> Found no questions</h2>
    }



    return (
            <form className={classes['question-list-form']}onSubmit={submitHandler}>
                <p  className={classes['question-list-form-progress']}>You answered: <span>{answersCount}/{props.items.length}</span></p>
                <div className={classes['question-list']}>
                    { props.items.map((question) => (
                            <QuestionItem
                            label={question.Label}
                            key={question.Id}
                            id={question.Id}
                            answers={question.Answers}
                            type={question.Type_id} 
                            AnsweredQuestionsHandler={AnsweredQuestionsHandler}   
                            commentHandler={commentHandler}      
                            />
                        ))
                    }
          </div>
          {showErrorMessage &&(<p className={classes['question-list__error']}>All questions are mandatory!</p>)}
          {!showScore&&<Button type="submit">Calculate</Button>}
          {showScore&&(<p className={classes['question-list__score']}>Score Calculation: <span>{score}</span></p>)}
    </form>
    );
  };
  
  export default QuestionList;