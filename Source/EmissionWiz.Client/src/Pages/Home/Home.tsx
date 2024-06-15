import React from 'react';
import { Report } from '../../Components/Report';
import { Col, Divider } from 'antd';
import { Link } from 'react-router-dom';

export default class Home extends React.Component {
    render(): React.ReactNode {
        return (
            <Report title="Быстрая навигация">
                <Col>
                    <h4>Методики</h4>
                    <Link to={'/single-source'} target={'_self'}>
                        Одиночный точечный источник
                    </Link>
                    <Divider />
                    <h4>Вещества</h4>
                    <Link to={'/substances'} target={'_self'}>
                        Вещества
                    </Link>
                </Col>
            </Report>
        );
    }
}
