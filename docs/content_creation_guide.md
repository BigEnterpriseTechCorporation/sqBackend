# Content Creation Guide: Adding Units and Exercises

This guide provides simple, step-by-step instructions for adding learning content to the SQL Learning Platform. No technical expertise required!

## Table of Contents

- [Getting Started](#getting-started)
- [Understanding Units and Exercises](#understanding-units-and-exercises)
- [Adding a New Learning Unit](#adding-a-new-learning-unit)
- [Creating SQL Exercises](#creating-sql-exercises)
- [Exercise Types Explained](#exercise-types-explained)
- [Tips for Creating Effective Content](#tips-for-creating-effective-content)

## Getting Started

### Prerequisites

Before you begin creating content, you'll need:

1. **Administrator access** to the SQL Learning Platform
2. Basic understanding of SQL concepts
3. A plan for your learning content structure

### Accessing the Content Management Interface

1. Log in to the SQL Learning Platform with your administrator account
2. Click on your profile icon in the top-right corner
3. Select "Admin Dashboard" from the dropdown menu
4. Click on "Content Management" in the sidebar

You'll now see the content management interface where you can add and manage units and exercises.

## Understanding Units and Exercises

The platform organizes content into a two-level hierarchy:

- **Units**: Top-level learning modules (e.g., "Introduction to SELECT Statements")
- **Exercises**: Individual SQL challenges within a unit (e.g., "Select all customers from New York")

A typical unit contains 5-10 exercises that follow a logical progression from basic to more advanced concepts.

## Adding a New Learning Unit

### Step 1: Create the Unit

1. From the Content Management page, click "Add New Unit"
2. Fill in the basic information:
   - **Title**: Clear, descriptive name (e.g., "Filtering Data with WHERE Clauses")
   - **Description**: Brief overview of what learners will accomplish
   - **Difficulty**: Select from Easy, Medium, Hard, or Ultra Hard
   - **Order**: Numeric position in the unit list (e.g., 1, 2, 3)
   - **Image** (optional): Upload a representative image

3. Click "Create Unit"

### Step 2: Add Unit Details

After creating the unit, you'll be taken to the unit detail page. Here you can:

1. Add learning objectives
   - Click "Add Learning Objective"
   - Enter a clear, action-oriented objective (e.g., "Use WHERE clauses to filter numeric data")
   - Click "Save"
   - Repeat for all learning objectives (3-5 recommended)

2. Add prerequisites (optional)
   - Click "Add Prerequisite"
   - Select from existing units or enter custom prerequisites
   - Click "Save"

3. Add introduction text
   - Use the rich text editor to write an introduction
   - Include explanations of key concepts
   - Consider adding code examples using the code formatting button

4. Click "Save Unit Details"

## Creating SQL Exercises

### Step 1: Add an Exercise to a Unit

1. From your unit detail page, click "Add New Exercise"
2. Fill in the basic information:
   - **Title**: Clear, descriptive name
   - **Description**: Instructions for the learner
   - **Difficulty**: Select from Easy, Medium, Hard, or Ultra Hard
   - **Type**: Choose the exercise type (explained below)
   - **Order**: Position within the unit (e.g., 1, 2, 3)

3. Click "Create Exercise"

### Step 2: Configure Exercise Details

Depending on the exercise type, you'll need to provide different information:

#### For "Write SQL Query" Exercise Type:

1. **Database Schema**: Provide the table structure
   ```sql
   CREATE TABLE customers (
     id INT PRIMARY KEY,
     name VARCHAR(100),
     city VARCHAR(50),
     state VARCHAR(2),
     spending_amount DECIMAL(10,2)
   );
   ```

2. **Test Data**: Add data for testing the solution
   ```sql
   INSERT INTO customers VALUES
   (1, 'John Smith', 'New York', 'NY', 250.75),
   (2, 'Mary Johnson', 'Los Angeles', 'CA', 120.50),
   (3, 'Robert Brown', 'Chicago', 'IL', 85.00);
   ```

3. **Expected Solution**: The correct SQL query
   ```sql
   SELECT * FROM customers WHERE state = 'NY';
   ```

4. **Check Type**: Choose how to validate solutions
   - **Result Match**: Compare output data
   - **Query Structure**: Check for specific syntax elements
   - **Exact Match**: Require exact query match

5. **Hints** (optional): Add helpful tips for struggling learners

6. Click "Save Exercise"

## Exercise Types Explained

### 1. Write SQL Query

Learners write a complete SQL query to solve a problem.

**Example**: "Write a query to select all customers from New York state."

### 2. Fill in the Blanks

Learners complete a partially written SQL query.

**Example**: "SELECT * FROM customers WHERE state = _______;"

### 3. Multiple Choice

Learners select the correct SQL query from options.

**Example**: "Which query selects all customers from New York?"
- A. SELECT * FROM customers WHERE state = 'NY';
- B. SELECT * FROM customers WHERE city = 'New York';
- C. SELECT * FROM customers WHERE state IS 'NY';

### 4. Fix the Error

Learners correct errors in a SQL query.

**Example**: "Fix the errors in this query: SELECT * FRUM customers WERE state = NY;"

## Tips for Creating Effective Content

### For Units

1. **Start with clear objectives**: What should learners be able to do after completing the unit?
2. **Organize logically**: Structure content from basic to advanced
3. **Keep it focused**: Each unit should cover a specific topic or concept
4. **Include real-world examples**: Show practical applications of SQL concepts

### For Exercises

1. **Clear instructions**: Be specific about what you want learners to do
2. **Realistic data**: Use data that resembles real-world scenarios
3. **Progressive difficulty**: Start with simpler exercises and gradually increase complexity
4. **Helpful hints**: For difficult exercises, provide hints that guide without giving away the answer
5. **Meaningful feedback**: Configure feedback messages that explain why an answer is incorrect

### Testing Your Content

Always test exercises before publishing:

1. Create the exercise
2. Click "Preview" to see it from a learner's perspective
3. Try submitting the correct solution and verify it passes
4. Try incorrect solutions to ensure proper feedback
5. Make adjustments as needed

## Managing Existing Content

### Editing Units and Exercises

1. From the Content Management page, find the unit or exercise
2. Click the "Edit" button (pencil icon)
3. Make your changes
4. Click "Save"

### Reordering Content

1. From the Content Management page, click "Reorder Units" or "Reorder Exercises"
2. Drag and drop items into the desired order
3. Click "Save Order"

### Deleting Content

1. Find the unit or exercise to delete
2. Click the "Delete" button (trash icon)
3. Confirm deletion when prompted

**Warning**: Deleting a unit will also delete all exercises within it. This action cannot be undone.

## Need Help?

If you encounter any issues while creating content:

1. Click the "Help" button in the admin interface
2. Check the [Developer Guide](developer_guide.md) for technical details
3. Contact the platform administrator at support@sqllearningplatform.com 