import React from 'react';
import { useState } from 'react';
import { jwtDecode } from "jwt-decode";
import logo from './logo.svg';
import './App.css';

function LoginComponent() {

  const initialState: any = null;
  const [parsedJwt, setParsedJwt] = useState(initialState);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [badLogin, setBadLogin] = useState(false);

  function login(this: XMLHttpRequest) {
    console.log('finished request:' + this);
    if(this.status >= 400 && this.status < 500) {
      console.log('bad login credentials');
      setBadLogin(true);
      return;
    }

    setBadLogin(false);
    setPassword("");//dont keep this in plaintext
    setParsedJwt(jwtDecode(this.response));
    localStorage.setItem("Authorization", this.response);
  }
  
  function logout() {
    setParsedJwt(initialState);
    localStorage.setItem("Authorization", "");
  }

  function submitLoginForm(e: any) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'http://identity.seasprig.dev/user/login?email='+username+'&password='+password);
    xhr.onload = login;
    xhr.send();
  }
  function handleUsernameChange(e: any) {
    setUsername(e.target.value);
  }
  function handlePasswordChange(e: any) {
    setPassword(e.target.value);
  }
  
  function LoginForm() {
    return (
      <div className="Login-form"> 
          <img src={logo} className="App-logo" alt="logo" />
          <p>SeaSprig</p>
          { badLogin && parsedJwt == null  ? <p>Invalid username or password</p> : null }
          <input type="text" value={username } onChange={handleUsernameChange} placeholder="username"/> 
          <input type="password" value={password} onChange={handlePasswordChange} placeholder="password"/>
          <button type="button" onClick={submitLoginForm}>Submit</button>
      </div>
    )
  
  }

  function ProfileCard() {
    return (
        <div>
          <p> Welcome, {parsedJwt.preferred_username}</p>
          <button type="button" onClick={logout}>Logout</button>
        </div>
    )
  }

  return (
    parsedJwt == null ? LoginForm() : ProfileCard()
  );

}

function App() {
  
  return (
        <LoginComponent />
  );
}

export default App;
