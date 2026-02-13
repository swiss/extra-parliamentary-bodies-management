export interface FormLettersSenderCreate {
    surname: string;
    givenName: string;
    senderFunctionId: string;
    departmentId: string;
    officeId: string;
    streetGerman: string;
    streetFrench: string;
    streetItalian: string;
    streetRomansh: string;
    zip: string;
    cityGerman: string;
    cityFrench: string;
    cityItalian: string;
    cityRomansh: string;
    phone?: string;
    email?: string;
    website?: string;
    signature?: File;
}
