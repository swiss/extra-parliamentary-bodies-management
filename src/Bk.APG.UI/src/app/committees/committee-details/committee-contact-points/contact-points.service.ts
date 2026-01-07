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
        return this.http.get<ContactPointList[]>(`/api/contactpoints/${committeeId}/list`);
    }

    getContactPointForUpdate(contactPointId: string): Observable<ContactPointUpdate> {
        return this.http.get<ContactPointUpdate>(`/api/contactpoints/${contactPointId}/update`);
    }

    getContactPointForCreate(committeeId: string): Observable<ContactPointCreate> {
        return this.http.get<ContactPointCreate>(`/api/contactpoints/${committeeId}/create`);
    }

    updateContactPoint(contactPoint: ContactPointUpdate): Observable<ContactPointUpdate> {
        return this.http.put<ContactPointUpdate>(`/api/contactpoints/${contactPoint.id}`, contactPoint);
    }

    createContactPoint = (contactPoint: ContactPointCreate) => this.http.post<ContactPointDetail>('/api/contactpoints', contactPoint);

    deleteContactPoint(contactPointId: string): Observable<ContactPointList[]> {
        return this.http.delete<ContactPointList[]>(`/api/contactpoints/${contactPointId}`);
    }
}
