import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import 'bootstrap/dist/css/bootstrap.min.css';
import './Scss/App.scss'

ReactDOM.createRoot(document.getElementById('react-app')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
