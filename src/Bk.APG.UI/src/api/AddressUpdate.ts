export interface AddressUpdate {
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
    cantonId?: string;
    activeAddress: boolean;
}
