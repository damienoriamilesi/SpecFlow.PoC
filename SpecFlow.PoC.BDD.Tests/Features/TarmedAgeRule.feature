Feature: TarmedAgeRule
	Simple calculator for adding two numbers

@tarmed-rule @valorization
Scenario: Check age compatibility of a service line from a billing request
	Given a service with position with age constraint
	And a patient who is outside the rule age 
	When I check the validity of the service
	Then I receive an Error message