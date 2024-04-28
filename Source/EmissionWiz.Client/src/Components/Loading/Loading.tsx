import * as React from 'react';
import { observer } from 'mobx-react';
import { GlobalProgressData, loaderStore } from '../../Stores/LoaderStore';
import Icon from '@ant-design/icons';
import { Progress } from 'antd';

type LoadingProps = {
    fullpage?: boolean;
    loading?: boolean;
    isSuspense?: boolean;
    small?: boolean;
    className?: string;
    iconStyle?: React.CSSProperties;
    progress?: GlobalProgressData | null;
};

@observer
export class Loading extends React.Component<LoadingProps, object> {
    render () {
        const { fullpage, loading, className, iconStyle, isSuspense, small } = this.props;
        const visible = loading || loaderStore.globalLoader.isPending || isSuspense;
        const classStr = className ? [className] : ['loader'];
        let style: React.CSSProperties = fullpage ? { position: 'fixed' } : { position: 'absolute' };
        if (isSuspense) {
            style = {};
            classStr.push('loader-suspense');
        }
        if (small) {
            classStr.push('loader-small');
        }
        return (
            <div hidden={!visible} className={classStr.join(' ')} style={style}>
                <Icon name="sync-alt" className="loader-icon" style={iconStyle}/>
                {this._renderProgress}
            </div>
        );
    }

    private get _renderProgress () {
        const { progress } = this.props;
        if (!progress) return null;
        return (
            <div className="loader-progress">
                <h3>{progress?.title}</h3>
                <Progress percent={((progress?.current ?? 0) / (progress?.total ?? 1)) * 100} className="progress-center-wrapper"/>
                <div className="loader-progress-label">
                    <span>
                        {progress?.current} / {progress?.total}
                    </span>
                </div>
            </div>
        );
    }
}