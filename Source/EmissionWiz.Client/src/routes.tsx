import React from "react";
import { AppContainer } from "./Components/AppContainer";
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import ErrorBoundary from "antd/es/alert/ErrorBoundary";
import { NavMenu } from "./Components/NavMenu";
import { Layout } from "./Components/Layout";
import Home from "./Pages/Home/Home";
import SingleSource from "./Pages/SingleSource/SingleSource";
import Substances from "./Pages/Substances/Substances";

export default class AppRoutes {
    static get routes() {
        return <AppContainer>{AppRoutes.getLayout()}</AppContainer>
    }

    private static getLayout() {
        const routes: { path: string, component: JSX.Element }[] = [];

        routes.push({ path: '/', component: <Home /> });
        routes.push({ path: '/single-source', component: <SingleSource /> })
        routes.push({ path: '/substances', component: <Substances /> })

        return (
            <React.Suspense>
                <ErrorBoundary>
                    <BrowserRouter>
                        <NavMenu />
                        <Routes>
                            {routes.map(x => <Route key={x.path} path={x.path} element={<Layout>{x.component}</Layout>} />)}
                        </Routes>
                    </BrowserRouter>
                </ErrorBoundary>
            </React.Suspense>
        )
    }
}