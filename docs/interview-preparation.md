# Interview preparation

Use this guide to verify that you—not just the repository—can defend the project. Answer each question aloud, then confirm your explanation against the linked code.

## Frontend foundations

1. Why is TanStack Query a better fit for claims than putting API responses in React context?
2. What belongs in a query key, and why do the dashboard filters belong there?
3. How do `isLoading` and `isFetching` create different user experiences?
4. Why use React Hook Form with Zod when the API already validates requests?
5. What prevents an unauthenticated user from opening a protected route?
6. Why is that route guard not a security boundary?
7. How are loading, empty, validation, unauthorized, and server-error states presented accessibly?
8. What would need to change if the table contained 100,000 claims?

## Security

1. Why can public registration create only an `Adjuster`?
2. Where is role authorization actually enforced?
3. What are the risks of storing a JWT in session storage?
4. How would an `HttpOnly` cookie design change the frontend and API?
5. How are passwords stored, and why is hashing different from encryption?
6. What validation is applied to uploaded documents, and what additional malware controls would production need?

## Backend and data

1. What responsibility belongs to each solution layer?
2. Which claim rules belong in the domain rather than the controller?
3. Why are read-only queries no-tracking?
4. How do unique database indexes differ from application validation?
5. Why can PostgreSQL and SQL Server behave differently behind the same EF Core model?
6. How would you make claim creation idempotent if clients retried requests?

## Delivery and operations

1. Explain every stage of the Dockerfile and why the final image excludes SDKs and Node.js.
2. Why serve the React build from ASP.NET Core for this deployment?
3. What checks run in CI, and which failure does each catch?
4. What happens when Render's free instance sleeps?
5. Why are document uploads not durable on the current public deployment?
6. How would you deploy this container on AWS using ECR, ECS Fargate or App Runner, RDS, and S3?

## Practical drills

- Add a new claim status and update every affected test.
- Change pagination size and explain the frontend and backend consequences.
- Diagnose a deliberately failed API request using browser network tools and server logs.
- Replace session storage with an in-memory session and explain the UX trade-off.
- Write one Playwright test without copying an existing test.
- Draw the request path from a dashboard click to PostgreSQL and back.

If an answer depends on memorized vocabulary, revisit the code and reproduce the behavior. A strong interview answer should state the requirement, chosen design, trade-off, and a credible alternative.
