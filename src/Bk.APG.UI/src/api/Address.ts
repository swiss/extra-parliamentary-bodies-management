import {Canton} from './Canton';
import {Country} from './Country';

export interface Address {
    id: string;
    companyName?: string;
    street?: string;
    poBox?: string;
    country: Country;
    zip?: string;
    city?: string;
    phone?: string;
    mobile?: string;
    email?: string;
    canton?: Canton;
    activeAddress: boolean;
}
