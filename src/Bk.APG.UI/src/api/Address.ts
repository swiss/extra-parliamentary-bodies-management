import {Canton} from './Canton';

export interface Address {
    id: string;
    companyName?: string;
    street?: string;
    poBox?: string;
    countryCode?: string;
    zip?: string;
    city?: string;
    phone?: string;
    mobile?: string;
    email?: string;
    canton?: Canton;
    activeAddress: boolean;
}
