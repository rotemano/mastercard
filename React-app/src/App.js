import React, { useState,useEffect } from "react";

import './App.css';
import Header from './components/Header/Header';
import QuestionList from './components/Questions/QuestionList';
  
function App() {
  const [questionnaire, setQuestionnaire]=useState([])
  const [port,setPort]=useState("58713");

  useEffect(() => {
    var URL="http://localhost:"+port+"/api/questions";
    fetch(URL,{
      mode: 'cors',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json', 
      }})
      .then(response => {
        return response.json();
      })
      .then(
        (data) => {
          setQuestionnaire(data);
        },
        (error) => {
          console.log(error)
        }
      )
  }, [])



  return (
    <React.Fragment>
      <Header />
      <QuestionList items={questionnaire} portNumber={port}></QuestionList>
    </React.Fragment>
  );
}

export default App;
