import * as ReactDOM from 'react-dom';
import App from './App.tsx'
import 'bootstrap/dist/css/bootstrap.min.css';
import './Scss/App.scss'
import { globalAjaxLoaderStore } from './Stores/GlobalAjaxLoaderStore.ts';
import { appStore } from './Stores/AppStore.tsx';

let devicePixelRatio: number = 0;
const resizeHandler = () => {
  if (!devicePixelRatio || devicePixelRatio !== window.devicePixelRatio) {
    devicePixelRatio = window.devicePixelRatio;

    const scrollDiv = document.createElement('div');
    scrollDiv.style.width = '100px';
    scrollDiv.style.height = '100px';
    scrollDiv.style.overflowY = 'scroll';
    scrollDiv.style.visibility = 'hidden';
    document.body.appendChild(scrollDiv);

    let scrollBarWidth = (scrollDiv.offsetWidth - scrollDiv.clientWidth);

    document.body.removeChild(scrollDiv);
    document.body.style.setProperty('--scrollbar-width', scrollBarWidth + 'px');
  }
};

const renderCallback = () => {
  const loader = document.getElementById('react-app-loader');
  const message = document.getElementById('react-app-loader-message');
  const notification = document.getElementById('react-notification-bar');
  const notificationMessage = document.getElementById('react-notification-bar-message');
  if (loader && message) {
    globalAjaxLoaderStore.appLoaderElement = loader;
    globalAjaxLoaderStore.appLoaderMessageElement = message;
    loader.addEventListener(
      'transitionend',
      () => {
        if (loader.classList.contains('hide')) {
          loader.classList.add('invisible');
        }
      },
      true
    );
  }
  if (notification) {
    appStore.notificationBar = notification;
    appStore.notificationBarMessage = notificationMessage;
  }
  resizeHandler();
}

const rootEl = document.getElementById('react-app')!;
ReactDOM.render(
  <App />,
  rootEl,
  renderCallback)
