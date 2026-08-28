# Contributing

Thanks for contributing! Please follow these guidelines to make collaboration smooth.

## Branching
- Base work on `main` and create feature branches with the pattern `feature/<ticket>-short-description` or `feature/<short-description>` if no ticket.
- Include the Redmine ticket number in the branch name when applicable (e.g., `feature/123-add-login`).

## Commit messages
- Use a short summary in the first line (50 chars or less), followed by a blank line and detailed description if needed.
- Include the Redmine ticket number in the commit message subject or body.

Example:
```
feat: Add login endpoint (Redmine#123)

- Implemented login
- Added unit tests
```

## Pull Requests
- Open a Pull Request from your feature branch into `main`.
- Use the PR template to explain the change, reference the Redmine ticket, and list test steps.
- At least one approving review is required before merge.

## Code Review
- Keep changes small and focused.
- Write unit tests for behavior changes.
- Ensure CI passes before merging.

