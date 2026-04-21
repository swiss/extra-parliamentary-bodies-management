# Changelog

## 1.4.0

- Export interests without begin date [BKDO-3453](https://bkdev.atlassian.net/browse/BKDO-3453)
- Add "Other" as election office [BKDO-3326](https://bkdev.atlassian.net/browse/BKDO-3326)
- Introduce information note report [BKDO-1032](https://bkdev.atlassian.net/browse/BKDO-1032)
- No general election for observers [BKDO-3524](https://bkdev.atlassian.net/browse/BKDO-3524)
- Fix form letter issues with empty salutations [BKDO-3525](https://bkdev.atlassian.net/browse/BKDO-3525)

## 1.3.0

- Add form letter export [BKDO-288](https://bkdev.atlassian.net/browse/BKDO-288)
- Show female function text for female person [BKDO-3266](https://bkdev.atlassian.net/browse/BKDO-3266)
- Add re-election to conditional end date validation [BKDO-3328](https://bkdev.atlassian.net/browse/BKDO-3328)
- Mark membership with end date today still as active [BKDO-3327](https://bkdev.atlassian.net/browse/BKDO-3327)
- Add eIAM assignment role validations [BKDO-2574](https://bkdev.atlassian.net/browse/BKDO-2574)
- Exclude inactive contact points from OGD export [BKDO-3290](https://bkdev.atlassian.net/browse/BKDO-3290)
- Case-insensitive sorting in committee membership table [BKDO-3289](https://bkdev.atlassian.net/browse/BKDO-3289)
- Update occupation labels when changing language [BKDO-3336](https://bkdev.atlassian.net/browse/BKDO-3336)
- Update membership addition label when changing language [BKDO-3337](https://bkdev.atlassian.net/browse/BKDO-3337)
- Update function labels [BKDO-3393](https://bkdev.atlassian.net/browse/BKDO-3393)
- Validation fixes (requirements profile, expired memberships, missing occupation includes, federal duty, federal assembly) [BKDO-3399](https://bkdev.atlassian.net/browse/BKDO-3399)

## 1.2.0

- Introduce Countries master data and add to address [BKDO-1103](https://bkdev.atlassian.net/browse/BKDO-1103)
- Fix race condition in committee justification tab [BKDO-3020](https://bkdev.atlassian.net/browse/BKDO-3020)
- Fix membership validation issue for disabled date inputs [BKDO-3084](https://bkdev.atlassian.net/browse/BKDO-3084)
- Enable begin date for admins (GEW committee) [BKDO-3088](https://bkdev.atlassian.net/browse/BKDO-3088)
- Update institution appointment decision type label [BKDO-3112](https://bkdev.atlassian.net/browse/BKDO-3112)
- Update and remove membership addition master data [BKDO-3095](https://bkdev.atlassian.net/browse/BKDO-3095)
- Extend GEW committee overview filters [BKDO-2090](https://bkdev.atlassian.net/browse/BKDO-2090)
- Open sourced NuGet Packages integrated (Swiss.FCh.Utils, Swiss.FCh.MasterData, Swiss.FCh.Monitoring) [BKDO-2859](https://bkdev.atlassian.net/browse/BKDO-2859)
- Remove obsolete ogd id from general measures [BKDO-3156](https://bkdev.atlassian.net/browse/BKDO-3156)
- Show estimated term of office in candidate list [BKDO-2089](https://bkdev.atlassian.net/browse/BKDO-2089)
- Stop copying (mirror) memberships for validated candidate list [BKDO-2060](https://bkdev.atlassian.net/browse/BKDO-2860)
- Invalidate candidate list when membership changes after validation [BKDO-2797](https://bkdev.atlassian.net/browse/BKDO-2797)
- Set flag and copy (mirror) memberships when memberships change after candidate list is ready for proposal [BKDO-2861](https://bkdev.atlassian.net/browse/BKDO-2861)
- Introduce ending of the GeneralElection [BKDO-1847](https://bkdev.atlassian.net/browse/BKDO-1847)
- Create new committee when GeneralElection is running [BKDO-972](https://bkdev.atlassian.net/browse/BKDO-972)
- Export male and female occupation and function text [BKDO-3290](https://bkdev.atlassian.net/browse/BKDO-3290)
- Cleanup and extend membership additions master data [BKDO-3106](https://bkdev.atlassian.net/browse/BKDO-3106)

## 1.1.1

- Form letter sender management [BKDO-1109](https://bkdev.atlassian.net/browse/BKDO-1109)
- Exclude CrossBorderFederalAgencies from OGD export [BKDO-2798](https://bkdev.atlassian.net/browse/BKDO-2798)
- Migrate "Anstalt" legal form to "Institut des öffentlichen Rechts" [BKDO-2798](https://bkdev.atlassian.net/browse/BKDO-2798)
- Mark OGD cubes as published for visualiziation [BKDO-2894](https://bkdev.atlassian.net/browse/BKDO-2894)
- Add general election ready for federal council proposal [BKDO-826](https://bkdev.atlassian.net/browse/BKDO-826)

## 1.1.0

- RDF Export with multiple Targets (Lindas and LindasNext) [BKDO-2501](https://bkdev.atlassian.net/browse/BKDO-2501)
- Candidate List Export added #BKDO-924
- Add task for general election person interest validation #BKDO-970
- Add task for general election person base data validation #BKDO-966
- Recipient Export added #BKDO-777

## 1.0.4

- Fix logic determining when shorter duty justification is needed #BKDO-2730

## 1.0.3

- Adapt handling of membership endDate and electionType #BKDO-2700

## 1.0.2

- Fixes notifications and overview for contact points #BKDO-2655
- Allow worklist task page for observers #BKDO-2627
- Add missing translations F/I #BKDO-2625

## 1.0.1

- Update contact point validators in frontend #BKDO-2620
