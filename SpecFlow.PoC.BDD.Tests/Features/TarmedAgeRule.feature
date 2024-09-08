Feature: TarmedAgeRule
	Simple calculator for adding two numbers

@tarmed-rule @valorization
Scenario: Check age compatibility of a service line from a billing request
	Given a service with age position constraint like '<positionCode>'
	And a patient whose age is outside the rule age of the position constraint : '<LimitAge>'
	When I check the validity of the service
	Then I receive an Error message
Examples: 
  	| positionCode | LimitAge |
    | 00.0040      | 6        |