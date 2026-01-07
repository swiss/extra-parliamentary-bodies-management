export interface ContactPointDetail {
    id: string;
    contactPointType: string;
    companyName: string;
    section: string;
    beginDate: Date;
    endDate: Date;
    street: string;
    poBox: string;
    zip: string;
    city: string;
    phone: string;
    email: string;
    surname: string;
    givenName: string;
    title: string;
    languageId: string;
    genderId: string;
    personalPhone: string;
    personalMobile: string;
    personalEmail: string;
    releasePersonData: boolean;
    isActive: boolean;
}
