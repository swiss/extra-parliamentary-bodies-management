export interface PersonList {
    id: string;
    surname: string;
    givenName: string;
    hasActiveMembership: boolean;
    birthYear: number;
    canton?: string;
    city?: string;
    language: string;
}
