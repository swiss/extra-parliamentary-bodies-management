export interface CommitteeTypeUpdate {
    id: string;
    text: string;
    femaleThreshold?: number;
    maleThreshold?: number;
    germanMinimalThreshold?: number;
    frenchMinimalThreshold?: number;
    italianMinimalThreshold?: number;
    romanshMinimalThreshold?: number;
    germanThresholdPercentage?: number;
    frenchThresholdPercentage?: number;
    italianThresholdPercentage?: number;
    romanshThresholdPercentage?: number;
}
