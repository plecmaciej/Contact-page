import React, { useState } from 'react';
import Login from './Login';
import Contacts from './Contacts';

const App = () => {
  const [token, setToken] = useState(localStorage.getItem('token'));

  const handleLogin = (newToken) => {
    setToken(newToken);
  };

  const handleLogout = () => {
    setToken(null);
    localStorage.removeItem('token'); // Remove token from localStorage
  };

  return (
    <div>
      <h1>Witaj w aplikacji</h1>

      {!token ? (
        <Login onLogin={handleLogin} />
      ) : (
        <button onClick={handleLogout}>Wyloguj</button>
      )}

      <Contacts token={token} />
  </div>
  );
};

export default App;
