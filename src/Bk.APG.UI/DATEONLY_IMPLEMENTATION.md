# DateOnly Implementation Guide

## Overview

This project uses C# `DateOnly` type on the backend for date-only fields (dates without time components). This document describes how we handle these dates in the Angular frontend.

## Architecture

### Type Definition
```typescript
// src/app/shared/DateAdapter.ts
export type DateOnly = string & {__brand: 'DateOnly'};
```

The `DateOnly` type is a branded string type representing dates in `yyyy-MM-dd` format, matching the C# `DateOnly` serialization.

### Automatic Conversion (Recommended Approach)

We use an HTTP interceptor (`DateOnlyInterceptor`) to automatically handle conversion between Date objects and DateOnly strings at the API boundary:

**Outgoing requests (Frontend → Backend):**
- Date objects are converted to `yyyy-MM-dd` strings
- Example: `new Date(2024, 0, 15)` → `"2024-01-15"`

**Incoming responses (Backend → Frontend):**
- `yyyy-MM-dd` strings are converted to Date objects
- ISO datetime strings (with time component) are left unchanged for Angular's HttpClient
- Example: `"2024-01-15"` → `new Date(2024, 0, 15)`

### Conversion Helpers

For manual conversion when needed (e.g., URL parameters, local storage):

```typescript
import {toDateOnlyString, fromDateOnlyString} from '@shared/DateAdapter';

// Date to DateOnly string
const dateOnlyStr = toDateOnlyString(new Date(2024, 0, 15));
// Result: "2024-01-15"

// DateOnly string to Date
const date = fromDateOnlyString("2024-01-15");
// Result: Date object representing Jan 15, 2024 in local time
```

### Material DatePicker Integration

The `ApgDateAdapter` in `DateAdapter.ts` handles parsing and formatting for Material datepickers:

- **User input format**: `dd.MM.yyyy` (Swiss format)
- **Display format**: `dd.MM.yyyy`
- **Backend format**: `yyyy-MM-dd` (handled by interceptor)

## Usage Patterns

### Components with Forms

Forms use `Date` objects directly. The interceptor handles conversion:

```typescript
// Component
form = this.formBuilder.group({
  beginDate: new FormControl<Date | undefined>(undefined),
  endDate: new FormControl<Date | undefined>(undefined),
});

// Service call - interceptor converts automatically
this.committeeService.create(this.form.getRawValue()).subscribe(...);
```

### TypeScript Models

Keep date fields as `Date` type in your TypeScript models for compatibility with Material datepickers:

```typescript
// src/api/CommitteeDetails.ts
export interface CommitteeDetails {
  beginDate: Date;  // Will be converted from/to "yyyy-MM-dd" by interceptor
  endDate?: Date;
  // ...
}
```

### URL Parameters

For date parameters in URLs, use `toDateOnlyString()`:

```typescript
import {toDateOnlyString} from '@shared/DateAdapter';

validateMembership(validationRequest: ValidationRequest) {
  let params = new HttpParams();
  if (validationRequest.beginDate) {
    params = params.append('beginDate', toDateOnlyString(validationRequest.beginDate));
  }
  return this.http.get('/api/validate', {params});
}
```

## Important Notes

### ⚠️ Never use `toISOString()` for DateOnly

`toISOString()` includes timezone conversion and time components, which causes date shifts:

```typescript
// ❌ WRONG - causes timezone issues
const date = new Date(2024, 0, 15);
date.toISOString(); // "2024-01-14T23:00:00.000Z" (shifted by timezone!)

// ✅ CORRECT - no timezone conversion
toDateOnlyString(date); // "2024-01-15"
```

### Date vs DateTime

- **DateOnly (no time)**: Use `Date` type in TS, serializes as `yyyy-MM-dd`
  - Examples: birthDate, beginDate, endDate
- **DateTime (with time)**: Use `Date` type in TS, serializes as ISO 8601 with time
  - Examples: created, modified, timestamp fields

The interceptor distinguishes between these based on the string format.

## Files Modified

### Core Implementation
- `src/app/shared/DateAdapter.ts` - Type definition and conversion helpers
- `src/app/interceptor/date-only.interceptor.ts` - HTTP interceptor
- `src/app/interceptor/date-only.interceptor.spec.ts` - Unit tests
- `src/main.ts` - Interceptor registration

### Updated Services
- `src/app/committees/committees.service.ts`
- `src/app/worklist/worklist.service.ts`
- `src/app/exports/requests-and-reports/requests-and-reports.service.ts`
- `src/app/exports/data-analysis/data-analysis.service.ts`

All `removeTime()` methods replaced with `toDateOnlyString()`.

## Testing

The interceptor includes comprehensive unit tests covering:
- Outgoing date conversion
- Incoming date conversion
- Array handling
- Null/undefined handling
- Nested object handling
- DateTime vs DateOnly string distinction

Run tests with:
```bash
npm test date-only.interceptor
```

## Migration Checklist

When working with new date fields:

- [ ] Use `Date` type in TypeScript models
- [ ] Use Material datepicker components for date inputs
- [ ] Let the interceptor handle HTTP serialization/deserialization
- [ ] Use `toDateOnlyString()` for URL parameters only
- [ ] Never use `toISOString()` for date-only fields
- [ ] Test timezone edge cases (date boundaries)
