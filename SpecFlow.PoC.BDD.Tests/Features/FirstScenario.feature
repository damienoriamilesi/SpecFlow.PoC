Feature: Check Insurance from Invoice

@invoice
Scenario: Avoid checking CaDa if not LAMal
Some more text:
- Scenario text 1
- Scenario text 2
- Scenario text 3
	
	https://docs.specflow.org/projects/specflow-livingdoc/en/latest/Generating/Markdown-and-Embedding-Images.html
	
	Given an invoice in an open status and is not Health insurance
	When I want to check the insured card data
	Then a business exception is raised with the message "This insurance is not LAMal"
	
@mytag
Scenario: Replace wrong insurance 
	![Calculator](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/Generating/Markdown-and-Embedding-Images.html)
	Given an invoice in an open status and has Health insurance
	When I check the insured card data by CaDa number
	Then I get the expected data 
	  | Id | IsLamal  | CreatedOn    | CreatedBy | UpdatedOn    | UpdatedBy |
	  | 42 | true     | "2023-01-09" | "Me"      | "2023-07-08" | "Me"      |
	  | 66 | false    | "2023-01-09" | "You"     | "2023-07-08" | "You"     |
   
@custom-tag
Scenario: Replace suggested disabled insurance
	Given an invoice in an open status and has Health insurance
	When the system suggests a disabled insurance
	Then I get an active insurance by the disabled GLN