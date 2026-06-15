import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ContactPointCreate} from '@api/ContactPointCreate';
import {ContactPointDetail} from '@api/ContactPointDetail';
import {ContactPointList} from '@api/ContactPointList';
import {ContactPointUpdate} from '@api/ContactPointUpdate';
import {Observable, Subject} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ContactPointsService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getContactPointList(committeeId: string): Observable<ContactPointList[]> {
        return this.http.get<ContactPointList[]>(`/api/contact-points/${committeeId}/list`);
    }

    getContactPointForUpdate(contactPointId: string): Observable<ContactPointUpdate> {
        return this.http.get<ContactPointUpdate>(`/api/contact-points/${contactPointId}/update`);
    }

    getContactPointForCreate(committeeId: string): Observable<ContactPointCreate> {
        return this.http.get<ContactPointCreate>(`/api/contact-points/${committeeId}/create`);
    }

    updateContactPoint(contactPoint: ContactPointUpdate): Observable<ContactPointUpdate> {
        return this.http.put<ContactPointUpdate>(`/api/contact-points/${contactPoint.id}`, contactPoint);
    }

    createContactPoint = (contactPoint: ContactPointCreate) => this.http.post<ContactPointDetail>('/api/contact-points', contactPoint);

    deleteContactPoint(contactPointId: string): Observable<ContactPointList[]> {
        return this.http.delete<ContactPointList[]>(`/api/contact-points/${contactPointId}`);
    }
}
