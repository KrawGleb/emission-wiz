import React from "react";
import { navigationStore } from "../Stores/NavigationStore";
import { Link } from "react-router-dom";
import { HoverContainer } from "./HoverContainer";

export class NavMenu extends React.Component {
    render() {

        return <div className="main-nav">
            <div className="main-nav-menu">
                <div className="main-nav-menu-left">
                    <div className="main-nav-group">
                        <span className="main-nav-group-name">{this._renderLink('/', 'Домой')}</span>
                    </div>
                    <HoverContainer className="main-nav-group" hoveredCssClass="menu-hover" title={<span className="main-nav-group-name">Методики</span>}>
                        <div className="main-nav-links">
                            {this._renderLink('/single-source', 'Одиночный точечный источник')}
                        </div>
                    </HoverContainer>
                    <div className="main-nav-group">
                        <span className="main-nav-group-name">{this._renderLink('/substances', 'Вещества')}</span>
                    </div>
                </div>
            </div>
        </div>
    }

    private _renderLink(toUrl: string | { pathname: string }, title: string, openInNewTab?: boolean, id?: string) {
        const isActive = navigationStore.currentLocation && navigationStore.currentLocation.pathname == toUrl;
        return (
            <Link key={title} to={toUrl} target={openInNewTab ? '_blank' : '_self'} className={'nav-link' + (isActive ? ' active' : '')} id={id}>
                {title}
            </Link>
        )
    }
}