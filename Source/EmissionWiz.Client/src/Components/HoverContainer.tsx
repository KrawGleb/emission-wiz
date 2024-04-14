import { action, observable } from "mobx";
import { observer } from "mobx-react";
import React from "react";

type ContextMenuItemProps = {
    title: string | JSX.Element;
    className?: string;
    style?: React.CSSProperties;
    hoveredCssClass?: string;
    showDelay?: number;
    hideDelay?: number;
    onShow?: () => void;
    onHide?: () => void;
    onClick?: (e: React.MouseEvent<HTMLDivElement>) => void;
    children: React.ReactNode;
};

@observer
export class HoverContainer extends React.Component<ContextMenuItemProps, {}> {
    @observable accessor isHovered: boolean = false;
    private _timer: number | null = null;
    private _delay: number = 300;

    render() {
        const { children, className, title, style, hoveredCssClass, onClick } = this.props;

        return <div className={className + ' ' + (hoveredCssClass ?? '')} style={style} onClick={onClick} onMouseOver={this._onMouseOver} onMouseOut={this._onMouseOut}>
            {title}
            {this.isHovered && children}
        </div>
    }

    @action.bound
    private _onMouseOver() {
        this._stopTimer();
        if (!this.isHovered) {
            const delay = this.props.showDelay !== undefined ? this.props.showDelay : this._delay;
            this._timer = window.setTimeout(action(() => {
                this.isHovered = true;
                this.props.onShow?.();
            }), delay);
        }
    }

    @action.bound
    private _onMouseOut() {
        this._stopTimer();
        if (this.isHovered) {
            const delay = this.props.hideDelay !== undefined ? this.props.hideDelay : this._delay;
            this._timer = window.setTimeout(action(() => {
                this.isHovered = false;
                this.props.onHide?.();
            }), delay);
        }
    }

    @action.bound
    private _stopTimer() {
        if (this._timer) {
            window.clearTimeout(this._timer);
            this._timer = null;
        }
    }
}