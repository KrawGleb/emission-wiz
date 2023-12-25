import { observable } from "mobx";

type JwtToken = {
    aio: string;
    amr: string[];
    aud: string;
    exp: number;
    family_name: string;
    given_name: string;
    iat: number;
    ipaddr: string;
    iss: string;
    name: string;
    nbf: number;
    nonce: string;
    oid: string;
    rh: string;
    roles: string[];
    sub: string;
    tid: string;
    unique_name: string;
    upn: string;
    uti: string;
    ver: string;
};

class AppStore {
    @observable accessor isAdaptive: boolean = false;
    @observable accessor bodyClassList: Set<string> = new Set([].slice.apply(document.body.classList));

}

export const appStore = new AppStore();