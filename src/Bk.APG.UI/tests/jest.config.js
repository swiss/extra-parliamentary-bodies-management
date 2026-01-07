'use strict';

/** @type {import('jest-preset-angular').Config} */
module.exports = {
    maxWorkers: 2,
    workerIdleMemoryLimit: '512MB',
    modulePathIgnorePatterns: ['<rootDir>/dist'],
    preset: 'jest-preset-angular',
    setupFilesAfterEnv: ['<rootDir>/tests/setupJest.ts'],
    testEnvironment: 'jsdom',
    moduleNameMapper: {
        '^@api/(.*)$': '<rootDir>/src/api/$1',
        '^@shared/(.*)$': '<rootDir>/src/app/shared/$1',
    },
};
